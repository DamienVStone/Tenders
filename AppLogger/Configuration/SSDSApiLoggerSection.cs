using AppLogger.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;

namespace AppLogger.Configuration
{
    class SSDSApiLoggerSection : ConfigurationSection
    {
        [ConfigurationProperty("url", IsRequired = true)]
        public string ApiUrl
        {
            get { return base["url"] as string; }
        }

        [ConfigurationProperty("log-level", IsRequired = true)]
        [TypeConverter(typeof(CaseInsensitiveEnumConfigConverter<LogType>))]
        public LogType LogLevel {
            get { return (LogType)base["log-level"]; }
        }

        [ConfigurationProperty("Loggers", IsRequired = true)]
        public LoggerCollection Loggers {
            get
            {
                return base["Loggers"] as LoggerCollection;
            }
        }

        [ConfigurationProperty("Timeouts", IsRequired = false)]
        public TimeoutConfig Timeout
        {
            get
            {
                return base["Timeouts"] as TimeoutConfig;
            }
        }

    }

    [ConfigurationCollection(typeof(LoggerConfig), AddItemName = "Logger")]
    class LoggerCollection : ConfigurationElementCollection, IEnumerable<LoggerConfig>
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new LoggerConfig();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var conf = element as LoggerConfig;
            if (conf != null)
                return conf.Username;
            else
                return null;
        }

        public LoggerConfig this[int index]
        {
            get
            {
                return BaseGet(index) as LoggerConfig;
            }
        }


        IEnumerator<LoggerConfig> IEnumerable<LoggerConfig>.GetEnumerator()
        {
            return (from i in Enumerable.Range(0, this.Count) select this[i]).GetEnumerator();
        }
    }

    class LoggerConfig : ConfigurationElement
    {
        [ConfigurationProperty("username", IsRequired = true)]
        public string Username
        {
            get
            {
                return base["username"] as string;
            }

        }

        [ConfigurationProperty("password", IsRequired = true)]
        public string Password
        {
            get
            {
                return base["password"] as string;
            }
            
        }
        
    }

    class TimeoutConfig : ConfigurationElement
    {
        [ConfigurationProperty("log", IsRequired = false, DefaultValue = 1000L)]
        public long? Log
        {
            get
            {
                return base["log"] as long?;
            }
        }

        [ConfigurationProperty("status", IsRequired = false, DefaultValue = 1000L)]
        public long? Status
        {
            get
            {
                return base["status"] as long?;
            }
        }

        [ConfigurationProperty("http", IsRequired = false, DefaultValue = 3000L)]
        public long? Http
        {
            get
            {
                return base["http"] as long?;
            }
        }
    }

    class CaseInsensitiveEnumConfigConverter<T> : ConfigurationConverterBase
    {
        public override object ConvertFrom(ITypeDescriptorContext ctx, CultureInfo ci, object data)
        {
            return Enum.Parse(typeof(T), (string)data, true);
        }
    }
}
