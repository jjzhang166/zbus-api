﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace zbus.Logging
{
    /// <summary>
    /// LogFactory Base class
    /// </summary>
    public class LoggerFactory
    {
        /// <summary>
        /// Gets the config file file path.
        /// </summary>
        protected string ConfigFile { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the server instance is running in isolation mode and the multiple server instances share the same logging configuration.
        /// </summary>
        protected bool IsSharedConfig { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogFactoryBase"/> class.
        /// </summary>
        /// <param name="configFile">The config file.</param>
        protected LoggerFactory(string configFile)
        {
            if (Path.IsPathRooted(configFile))
            {
                ConfigFile = configFile;
                return;
            }

            if (Path.DirectorySeparatorChar != '\\')
            {
                configFile = Path.GetFileNameWithoutExtension(configFile) + ".unix" + Path.GetExtension(configFile);
            }

            var currentAppDomain = AppDomain.CurrentDomain;
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFile);

            if (File.Exists(filePath))
            {
                ConfigFile = filePath;
                return;
            }

            filePath = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config"), configFile);

            if (File.Exists(filePath))
            {
                ConfigFile = filePath;
                return;
            }

            ConfigFile = configFile;
            return;
        }

        /// <summary>
        /// Gets the log by name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static ILogger GetLogger(string name)
        {
            return new ConsoleLogger(name);
        }

        public static ILogger GetLogger(Type type)
        {
            return GetLogger(type.Name);
        }
    }
}
