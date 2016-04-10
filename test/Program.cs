using HlLib.VersionControl;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    public class Program
    {
        static void Main(string[] args)
        {
            var config = new LoggingConfiguration();

            var consoleOutput = new ConsoleTarget()
            {
                Layout = @"[${longdate}] ${uppercase:${level:padding=-5}} ${logger} ${callsite} ${message}",
                Error = false,
            };
            config.AddTarget("console", consoleOutput);
            var consoleOutputRule = new LoggingRule("*", LogLevel.Trace, consoleOutput);
            consoleOutputRule.DisableLoggingForLevel(LogLevel.Error);
            consoleOutputRule.DisableLoggingForLevel(LogLevel.Fatal);
            config.LoggingRules.Add(consoleOutputRule);

            var consoleError = new ConsoleTarget()
            {
                Layout = @"[${longdate}] ${uppercase:${level:padding=-5}} ${logger} ${callsite} ${message}",
                Error = true,
            };
            config.AddTarget("console", consoleError);
            var consoleErrorRule = new LoggingRule("*", LogLevel.Error, consoleError);
            config.LoggingRules.Add(consoleErrorRule);

            var file = new FileTarget()
            {
                Layout = "[${longdate}]\t${uppercase:${level}}\t${logger}\t${callsite}\t${message}",
                FileName = Path.Combine(Environment.CurrentDirectory, "logs", "log.txt"),
                Encoding = Encoding.UTF8,
                CreateDirs = true,
                ArchiveNumbering = ArchiveNumberingMode.DateAndSequence,
                ArchiveAboveSize = 1 * 1024 * 1024 * 1024, // 1GB
            };
            config.AddTarget("file", file);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, file));

            var database = new DatabaseTarget()
            {
                DBHost = "localhost",
                DBUserName = "root",
                DBPassword = "",
                DBDatabase = "test",
                DBProvider = "MySql.Data.MySqlClient",
                CommandType = CommandType.Text,
                CommandText = 
                    @"insert into `logs`" +
                    @"  (`level`, `type`, `callsite`, `message`, `date`)" +
                    @"  values (@level, @type, @callsite, @message, @date)",
            };
            database.Parameters.Add(new DatabaseParameterInfo("@level", new SimpleLayout("${lowercase:${level}}")));
            database.Parameters.Add(new DatabaseParameterInfo("@type", new SimpleLayout("${logger}")));
            database.Parameters.Add(new DatabaseParameterInfo("@callsite", new SimpleLayout("${callsite}")));
            database.Parameters.Add(new DatabaseParameterInfo("@message", new SimpleLayout("${message}")));
            database.Parameters.Add(new DatabaseParameterInfo("@date", new SimpleLayout("${longdate}")));
            config.AddTarget("database", database);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, database));

            LogManager.Configuration = config;

            var logger = LogManager.GetCurrentClassLogger();
            var begin = DateTime.Now;
            logger.Trace("trage");
            logger.Debug("debug");
            logger.Info("info");
            logger.Warn("warn");
            logger.Error("error");
            logger.Fatal("fatal");
            Console.WriteLine(DateTime.Now - begin);

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                logger.Fatal(e.ExceptionObject as Exception);
            };
            //throw new Exception("aaa");

            Console.Read();
        }
    }
}
