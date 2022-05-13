using System;

namespace MySqlEntityCore.Template
{

    /// <summary>Track latest create &amp; write date.</summary>
    public class TrackingModel : Template.DefaultModel
    {

        [Field(Required = true, Writeable = false)]
        public DateTime CreateDate { get; set; }

        [Field(Required = true)]
        public DateTime WriteDate { get; set; }

        public new void Create()
        {
            CreateDate = DateTime.UtcNow;
            WriteDate = DateTime.UtcNow;
            base.Create();
        }

        public new void Write()
        {
            WriteDate = DateTime.UtcNow;
            base.Write();
        }

    }

}
