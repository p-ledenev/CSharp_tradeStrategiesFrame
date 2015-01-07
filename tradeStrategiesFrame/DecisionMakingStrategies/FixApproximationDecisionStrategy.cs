using System;
using tradeStrategiesFrame.Model;

namespace tradeStrategiesFrame.DecisionMakingStrategies
{
    class FixApproximationDecisionStrategy : ApproximationDecisionStrategy
    {
        public FixApproximationDecisionStrategy(Machine pft)
            : base(pft)
        {
            Candle.keys = new String[] { "k", "kx+b", "appN" };
        }

        protected override bool shouldContinueApproximationSearch(Approximation ap, int start)
        {
            return false;
        }

        protected override Approximation combine(Approximation toReturn, Approximation current)
        {
            //toReturn.requisities[Approximation.keys[0]] = current.requisities[Approximation.keys[0]];
            //toReturn.requisities[Approximation.keys[1]] = current.requisities[Approximation.keys[1]];
            //toReturn.requisities[Approximation.keys[2]] = current.requisities[Approximation.keys[2]];

            return toReturn;
        }

        protected override void setCandleRequisities(Approximation ap, int start)
        {
            pft.portfolio.candles[start].addRequisitieByKeyIndex(0, Math.Round(ap.k[0], 4).ToString());//ap.countResidual(pft.parent.siftedValues, start).ToString());
            pft.portfolio.candles[start].addRequisitieByKeyIndex(1, ap.printPowerFunc());
            pft.portfolio.candles[start].addRequisitieByKeyIndex(2, ap.countFunctionFor().ToString());
        }

        public override void readParamsFrom(String xml)
        {
        }
    }
}
