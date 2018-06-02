using System;
using System.Text.RegularExpressions;
using NDesk.DBus;
using Saturnin.Helpers;

namespace Saturnin.Interfaces
{
    public class DBusConnectionHandler
    {
        protected Connection conn;

        public DBusConnectionHandler(Connection conn)
        {
            this.conn = conn;
        }

        public void Handle()
        {
            Handle(conn);
        }

        public static void Handle(Connection conn)
        {
            string myName = "org.pirati.saturnin";
            ObjectPath myOpath = new ObjectPath("/org/pirati/saturnin");

            ControlSaturnin demo = new ControlSaturnin();
            conn.Register(myName, myOpath, demo);
            //conn.Register(myName, myOpath, new Introspectable());

            //TODO: handle lost connections etc. properly instead of stupido try/catch
            try
            {
                while (true)
                    conn.Iterate();
            }
            catch (Exception e)
            {
                Log.Write($"{e.Message} | StackTrace: {e.StackTrace}", Log.LogLevel.ERROR);
            }

            conn.Unregister(myName, myOpath);
        }
    }

    public interface IControlSaturnin
    {
        void Ahoj(string arg0);
    }

    [Interface("org.pirati.saturnin")]
    public class ControlSaturnin : IControlSaturnin
    {
        public void Ahoj(string arg0)
        {
            if (Regex.Match(arg0, @"^(\+[0-9]{9})$").Success)
            {
                Log.Write($"Triggered Saturnin DBus hello for recipient '{arg0}'");
            }
            else
            {
                Log.Write($"Triggered Saturnin DBus hello for invalid recipient '{arg0}'", Log.LogLevel.WARN);
            }
        }
    }

    [Interface("org.freedesktop.DBus.Introspectable")]
    public interface IIntrospectable
    {
        [return: Argument("data")]
        string Introspect();
    }

    [Interface("org.freedesktop.DBus.Introspectable")]
    public class Introspectable : IIntrospectable
    {
        public string Introspect()
        {
            return "<!DOCTYPE node PUBLIC \"-//freedesktop//DTD D-BUS Object Introspection 1.0//EN\" \"http://www.freedesktop.org/standards/dbus/1.0/introspect.dtd\">\n" +
                    "<node name=\"/org/pirati/saturnin\">\n" +
                    " <interface name=\"org.pirati.saturnin\">\n" +
                    "  <method name=\"Ahoj\" >\n" +
                    "   <arg type=\"s\" direction=\"in\"/>\n" +
                    "  </method>\n" +
                    " </interface>\n" +
                    " <interface name=\"org.freedesktop.DBus.Introspectable\">\n" +
                    "  <method name=\"Introspect\">\n" +
                    "   <arg type=\"s\" direction=\"out\"/>\n" +
                    "  </method>\n" +
                    " </interface>\n" +
                    "</node>\n";
        }
    }
}
