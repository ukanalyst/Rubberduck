﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Rubberduck.Inspections;
using Rubberduck.UI;

namespace Rubberduck.Settings
{
    public interface ICodeInspectionSettings
    {
        HashSet<CodeInspectionSetting> CodeInspections { get; set; }
        WhitelistedIdentifierSetting[] WhitelistedIdentifiers { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class CodeInspectionSettings : ICodeInspectionSettings
    {
        [XmlArrayItem("CodeInspection", IsNullable = false)]
        public HashSet<CodeInspectionSetting> CodeInspections { get; set; }

        [XmlArrayItem("WhitelistedIdentifier", IsNullable = false)]
        public WhitelistedIdentifierSetting[] WhitelistedIdentifiers { get; set; }

        public CodeInspectionSettings() : this(new HashSet<CodeInspectionSetting>(), new WhitelistedIdentifierSetting[] {})
        {
        }

        public CodeInspectionSettings(HashSet<CodeInspectionSetting> inspections, WhitelistedIdentifierSetting[] whitelistedNames)
        {
            CodeInspections = inspections;
            WhitelistedIdentifiers = whitelistedNames;
        }

        public CodeInspectionSetting GetSetting(Type inspection)
        {
            var proto = Convert.ChangeType(Activator.CreateInstance(inspection), inspection);
            var existing = CodeInspections.FirstOrDefault(s => s.Name.Equals(proto.GetType().ToString()));
            if (existing != null) return existing;
            var setting = new CodeInspectionSetting(proto as IInspectionModel);
            CodeInspections.Add(setting);
            return setting;
        }
    }

    [XmlType(AnonymousType = true)]
    public class CodeInspectionSetting : IInspectionModel
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlIgnore]
        public string Description { get; set; } // not serialized because culture-dependent

        [XmlIgnore]
        public string LocalizedName
        {
            get
            {
                return InspectionsUI.ResourceManager.GetString(Name + "Name", UI.Settings.Settings.Culture);
            }
        } // not serialized because culture-dependent

        [XmlIgnore]
        public string AnnotationName { get; set; }

        [XmlIgnore]
        public CodeInspectionSeverity DefaultSeverity { get; private set; }

        [XmlAttribute]
        public CodeInspectionSeverity Severity { get; set; }

        [XmlIgnore]
        public string Meta
        {
            get
            {
                return InspectionsUI.ResourceManager.GetString(Name + "Meta", UI.Settings.Settings.Culture);
            }
        }

        [XmlIgnore]
        public string TypeLabel
        {
            get { return RubberduckUI.ResourceManager.GetString("CodeInspectionSettings_" + InspectionType, UI.Settings.Settings.Culture); }
        }

        [XmlIgnore]
        public string SeverityLabel
        {
            get { return RubberduckUI.ResourceManager.GetString("CodeInspectionSeverity_" + Severity, UI.Settings.Settings.Culture); }
            set
            {
                foreach (var severity in Enum.GetValues(typeof (CodeInspectionSeverity)))
                {
                    if (value == RubberduckUI.ResourceManager.GetString("CodeInspectionSeverity_" + severity, UI.Settings.Settings.Culture))
                    {
                        Severity = (CodeInspectionSeverity)severity;
                        return;
                    }
                }
            }
        }

        [XmlAttribute]
        public CodeInspectionType InspectionType { get; set; }

        public CodeInspectionSetting()
        {
            //default constructor required for serialization
        }

        public CodeInspectionSetting(string name, string description, CodeInspectionType type, CodeInspectionSeverity defaultSeverity, CodeInspectionSeverity severity)
        {
            Name = name;
            Description = description;
            InspectionType = type;
            Severity = severity;
            DefaultSeverity = defaultSeverity;
        }

        public CodeInspectionSetting(IInspectionModel inspection)
            : this(inspection.Name, inspection.Description, inspection.InspectionType, inspection.DefaultSeverity, inspection.Severity)
        { }

        public override bool Equals(object obj)
        {
            var inspectionSetting = obj as CodeInspectionSetting;

            return inspectionSetting != null &&
                   inspectionSetting.InspectionType == InspectionType &&
                   inspectionSetting.Name == Name &&
                   inspectionSetting.Severity == Severity;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int) Severity;
                hashCode = (hashCode * 397) ^ (int) InspectionType;
                return hashCode;
            }
        }
    }
}
