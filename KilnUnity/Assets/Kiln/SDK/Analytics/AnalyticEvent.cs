namespace Kiln
{
    public interface IAnalyticEvent
    {
        string Category { get; set; }
        string Action { get; set; }
        string Label { get; set; }
        string Value { get; set; }

        string getCategory();
        string getAction();
        string getLabel();
        string getValue();
    }

    public class AnalyticEvent : IAnalyticEvent
    {
        public string Category { get; set; }

        public string Action { get; set; }

        public string Label { get; set; }

        public string Value { get; set; }

        public string getCategory() {
            return Category;
        }
        public string getAction() {
            return Action;
        }
        public string getLabel() {
            return Label;
        }
        public string getValue() {
            return Value;
        }

    }
}