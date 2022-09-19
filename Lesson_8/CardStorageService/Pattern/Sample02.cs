using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pattern
{
    internal class Sample02
    {
        static void Main(string[] args)
        {
            #region Singleton
            GlobalSettings settings = LazySingleton.Instance;
            #endregion

            #region Builder
            MailMessageBuilder
                .Create()
                .From("sample@gmail1.com")
                .To("sample@gmail2.com")
                .Body("some messege")
                .Build();
            #endregion
        }
    }

    #region Singleton Lazy

    public class GlobalSettings
    {
        public string Settings1 { get; set; }
        public string Settings2 { get; set; }
        public string Settings3 { get; set; }
    }
    public class LazySingleton
    {
        private static GlobalSettings GetGlobalSettings()
        {
            return new GlobalSettings();
        }
        private static readonly Lazy<GlobalSettings> _instance = new Lazy<GlobalSettings>(
            /*delegate () {return new GlobalSettings();}*/ // или так
            () => new GlobalSettings(),
            false);
        // создан анонимный метод, возвращающий значение GlobalSettings
        // Lazy<T> Обеспечивает поддержку отложенной инициализации. Lazy создаст единожды экземпляр _instance. Затем будет возвращать его каждый раз по
        // запросу.
        public static GlobalSettings Instance
        {
            get { return _instance.Value; }
        }
    }

    #endregion

    #region Builder
    public class MailMessage
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        // Представим что полей очень много и многие из них нужно получить в результате дополнительных вычислений с передачей множества параметров.
    }

    public class MailMessageBuilder
    {
        private readonly MailMessage _mailMessage = new MailMessage();
        public static MailMessageBuilder Create()
        {
            return new MailMessageBuilder();
        }
        public MailMessage Build()
        {
            return _mailMessage;
        }
        public MailMessageBuilder From(string address)
        {
            _mailMessage.From = address;
            return this;
        }
        public MailMessageBuilder To(string address)
        {
            _mailMessage.To = address;
            return this;
        }
        public MailMessageBuilder Subject(string subject)
        {
            _mailMessage.Subject = subject;
            return this;
        }
        public MailMessageBuilder Body(string body)
        {
            _mailMessage.Body = body;
            return this;
        }
    }


    #endregion




}
