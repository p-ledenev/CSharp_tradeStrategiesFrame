using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using tradeStrategiesFrame.DecisionMakingStrategies;
using tradeStrategiesFrame.Model;

namespace tradeStrategiesFrame.Factories
{
    class DecisionStrategyFactory
    {
        public static DecisionStrategy createDecisionStrategie(String decisionStrategyName, Machine machine)
        {
            List<Type> types = loadAllDecisionStrategies();

            foreach (Type type in types)
            {
                DecisionStrategy strategy = (DecisionStrategy)Activator.CreateInstance(type);

                if (!strategy.getName().Equals(decisionStrategyName))
                    continue;

                strategy.initWith(machine);
                return strategy;
            }

            throw new Exception("Decision strategy with name " + decisionStrategyName + " not found");
        }

        protected static List<Type> loadAllDecisionStrategies()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            return assembly.GetTypes().Where(type => "DecisionMakingStrategies".Equals(type.Namespace) && !type.IsAbstract).ToList();
        }
    }
}
