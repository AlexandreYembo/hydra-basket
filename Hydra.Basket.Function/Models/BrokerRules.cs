namespace Hydra.Basket.Function.Models
{
    public class BrokerRules
    {
        public BrokerRules(string msg){
            Message = msg;
        }
        public string Message { get; set; }
    }
}