using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmtpSender
{
    public class MailSettings
    {
        #region Ctor
        public MailSettings()
        { }
        #endregion Ctor

        #region Data
        List<string> attachments;
        BodyBuilder builder;
        #endregion Data

        #region Methods
        public bool AddAttachment(Stream data, string name, string content)
        {
            if (builder == null)
                builder = new BodyBuilder();
            try
            {
                builder.Attachments.Add(name, data, ContentType.Parse(content));
            }
            catch
            {
                return false;
            }
            return true;
        }
        public AttachmentCollection GetAttachments()
        {
            if (builder != null)
                return builder.Attachments;
            return null;
        }
        public void AddAttachmentFile(string path)
        {
            if (attachments == null)
                attachments = new List<string>();
            attachments.Add(path);
        }
        public int GetAttachmentFileCount()
        {
            if (attachments == null)
                attachments = new List<string>();
            return attachments.Count;
        }
        public List<string> GetAttachmentsFile()
        {
            if (attachments == null)
                attachments = new List<string>();
            return attachments;
        }
        #endregion

        #region Properties
        List<string> _Address;
        public List<string> Address 
        { 
            get 
            {
                if (_Address == null)
                    _Address = new List<string>();
                return _Address;
            }
        }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }

        public string Name { get; set; }
        public string Reason { get; set; }
        
        #endregion Properties
    }
}
