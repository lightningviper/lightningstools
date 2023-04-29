using F4SharedMem.Headers;
using System.Xml;

namespace F4SharedMemTester
{
    internal class IntellivibeDataXmlDeserializer
    {
        public static IntellivibeData Deserialize(XmlElement intellivibeDataXmlElement)
        {
            IntellivibeData intellivibeDataStruct = new IntellivibeData();
            if (intellivibeDataXmlElement == null) return intellivibeDataStruct;
            byte.TryParse(intellivibeDataXmlElement.GetAttribute("AAMissileFired"), out intellivibeDataStruct.AAMissileFired);
            byte.TryParse(intellivibeDataXmlElement.GetAttribute("AGMissileFired"), out intellivibeDataStruct.AGMissileFired);
            byte.TryParse(intellivibeDataXmlElement.GetAttribute("BombDropped"), out intellivibeDataStruct.BombDropped);
            byte.TryParse(intellivibeDataXmlElement.GetAttribute("FlareDropped"), out intellivibeDataStruct.FlareDropped);
            byte.TryParse(intellivibeDataXmlElement.GetAttribute("ChaffDropped"), out intellivibeDataStruct.ChaffDropped);
            byte.TryParse(intellivibeDataXmlElement.GetAttribute("BulletsFired"), out intellivibeDataStruct.BulletsFired);
            int.TryParse(intellivibeDataXmlElement.GetAttribute("CollisionCounter"), out intellivibeDataStruct.CollisionCounter);
            bool.TryParse(intellivibeDataXmlElement.GetAttribute("IsFiringGun"), out intellivibeDataStruct.IsFiringGun);
            bool.TryParse(intellivibeDataXmlElement.GetAttribute("IsEndFlight"), out intellivibeDataStruct.IsEndFlight);
            bool.TryParse(intellivibeDataXmlElement.GetAttribute("IsEjecting"), out intellivibeDataStruct.IsEjecting);
            bool.TryParse(intellivibeDataXmlElement.GetAttribute("In3D"), out intellivibeDataStruct.In3D);
            bool.TryParse(intellivibeDataXmlElement.GetAttribute("IsPaused"), out intellivibeDataStruct.IsPaused);
            bool.TryParse(intellivibeDataXmlElement.GetAttribute("IsFrozen"), out intellivibeDataStruct.IsFrozen);
            bool.TryParse(intellivibeDataXmlElement.GetAttribute("IsOverG"), out intellivibeDataStruct.IsOverG);
            bool.TryParse(intellivibeDataXmlElement.GetAttribute("IsOnGround"), out intellivibeDataStruct.IsOnGround);
            bool.TryParse(intellivibeDataXmlElement.GetAttribute("IsExitGame"), out intellivibeDataStruct.IsExitGame);
            float.TryParse(intellivibeDataXmlElement.GetAttribute("Gforce"), out intellivibeDataStruct.Gforce);
            float.TryParse(intellivibeDataXmlElement.GetAttribute("eyex"), out intellivibeDataStruct.eyex);
            float.TryParse(intellivibeDataXmlElement.GetAttribute("eyey"), out intellivibeDataStruct.eyey);
            float.TryParse(intellivibeDataXmlElement.GetAttribute("eyez"), out intellivibeDataStruct.eyez);
            int.TryParse(intellivibeDataXmlElement.GetAttribute("lastdamage"), out intellivibeDataStruct.lastdamage);
            float.TryParse(intellivibeDataXmlElement.GetAttribute("damageforce"), out intellivibeDataStruct.damageforce);
            int.TryParse(intellivibeDataXmlElement.GetAttribute("whendamage"), out intellivibeDataStruct.whendamage);

            return intellivibeDataStruct;
        }

    }
}

