namespace FarmUp.Models
{
    public class AnswersModel
    {
        public int q_ID { get; set; }
        public int q_usr_ID { get; set; }
        public string? ans_question { get; set; }
        public string? ans_answer { get; set; }
        public string? ans_from { get; set; }
    }
    public class AdsModel
    {
        public string ads_id { get; set; }
        public string imgUrl { get; set; }
        public string title { get; set; }
        public decimal price { get; set; }
    }
    public class AnswerList
    {
        public List<AnswersModel> AnswersModelList { get; set; } = new List<AnswersModel>();
        public List<AnswersModel> AnswersModelExtraList { get; set; } = new List<AnswersModel>();
        public List<AdsModel> AdsModelList { get; set; } = new List<AdsModel>();
    }
    
}
