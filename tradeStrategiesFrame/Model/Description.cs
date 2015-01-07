using System;
using System.Collections.Generic;
using System.Linq;

namespace tradeStrategiesFrame.Model
{
    class Description
    {
        public static String[] keys { get; set; }

        public Dictionary<String, String> requisites { get; set; }

        public Description()
        {
            requisites = new Dictionary<String, String>();
        }

        public void addRequisitieByKeyIndex(int index, String value)
        {
            requisites[keys[index]] = value;
        }

        public String printDescription()
        {
            if (keys == null)
                return "";

            String response = "";
            foreach (String key in keys)
                response += (requisites.Keys.Contains(key) ? requisites[key] : " ") + "|";

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
