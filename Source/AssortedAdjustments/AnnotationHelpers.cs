using System;

namespace AssortedAdjustments
{
    [AttributeUsage(AttributeTargets.All)]
    public class Annotation : Attribute
    {
        private string description;
        private string defaultValue;
        private bool startSection;
        private bool endSection;
        private string sectionLabel;

        public Annotation(string description, string defaultValue, bool startSection = false, string sectionLabel = "", bool endSection = false)
        {
            this.Description = description;
            this.DefaultValue = defaultValue;
            this.StartSection = startSection;
            this.SectionLabel = sectionLabel;
            this.EndSection = endSection;
        }

        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }

        public string DefaultValue
        {
            get
            {
                return this.defaultValue;
            }
            set
            {
                this.defaultValue = value;
            }
        }

        public bool StartSection
        {
            get
            {
                return this.startSection;
            }
            set
            {
                this.startSection = value;
            }
        }

        public string SectionLabel
        {
            get
            {
                return this.sectionLabel;
            }
            set
            {
                this.sectionLabel = value;
            }
        }

        public bool EndSection
        {
            get
            {
                return this.endSection;
            }
            set
            {
                this.endSection = value;
            }
        }
    }
}
