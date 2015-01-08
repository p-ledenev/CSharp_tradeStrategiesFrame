using System;
using tradeStrategiesFrame.ApproximationConstructors;
using tradeStrategiesFrame.Factories;
using tradeStrategiesFrame.Model;

namespace tradeStrategiesFrame.DecisionMakingStrategies
{
    class ApproximationDecisionStrategy : DecisionStrategy
    {
        public Approximation[] approximations { get; set; }

        public ApproximationDecisionStrategy(Machine machine)
            : base(machine)
        {
            initApproximations();
        }

        protected override Position.Direction determineTradeDirection(int start)
        {
            ApproximationConstructor constructor = ApproximationConstructorFactory.createConstructor();
            Approximation ap = constructor.approximate(machine.getCandles(), start, machine.minDepth);

            approximations[start] = ap;

            return determineTradeDirection(ap);
        }

        protected override void addAncillaryRequisites(int start)
        {
            Approximation ap = approximations[start];

            machine.addCandleRequisite("k", Math.Round(ap.k[0], 4).ToString(), start);
            machine.addCandleRequisite("kx+b", ap.printPowerFunc(), start);
            machine.addCandleRequisite("approximatedValue", ap.countFunctionFor().ToString(), start);
        }

        public override void readParamsFrom(string xml)
        {
        }

        protected Position.Direction determineTradeDirection(Approximation ap)
        {
            if (ap == null || ap.k == null) 
                return Position.Direction.None;

            if (ap.k[0] > 0) return Position.Direction.Buy;
            if (ap.k[0] < 0) return Position.Direction.Sell;

            return Position.Direction.None;
        }

        public void initApproximations()
        {
            approximations = new Approximation[machine.portfolio.candles.Length];

            for (int i = 0; i < approximations.Length; i++)
                approximations[i] = new Approximation();
        }
    }
}
