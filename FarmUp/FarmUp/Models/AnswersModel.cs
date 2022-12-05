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
    public class AnswerList
    {
        public List<AnswersModel> AnswersModelList { get; set; } = new List<AnswersModel>();
        public List<AnswersModel> AnswersModelExtraList { get; set; } = new List<AnswersModel>();
    }
    
}
