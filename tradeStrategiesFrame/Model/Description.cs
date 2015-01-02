using System;
using System.Collections.Generic;
using System.Linq;

namespace tradeStrategiesFrame.Model
{
    class Description
    {
        public static String[] keys { get; set; }

        public Dictionary<String, String> requisities { get; set; }

        public Description()
        {
            requisities = new Dictionary<String, String>();
        }

        public void addRequisitieByKeyIndex(int index, String value)
        {
            requisities[keys[index]] = value;
        }

        public String printDescription()
        {
            if (keys == null)
                return "";

            String response = "";
            foreach (String key in keys)
                response += (requisities.Keys.Contains(key) ? requisities[key] : " ") + "|";

            return response;
        }

        public static String printDescriptionHead()
        {
            if (keys == null)
                return "";

            String response = "";
            foreach (String key in keys)
                response += key + "|";

            return response;
        }
    }
}
