namespace Xbim.ExpressParser.SDAI
{
    public abstract class VariableSizeAggregationType: AggregationType
    {
        protected VariableSizeAggregationType()
        {
            LowerBound = -1;
        }

        public int LowerBound { get; set; }
        public int? UpperBound { get; set; }
    }
}
