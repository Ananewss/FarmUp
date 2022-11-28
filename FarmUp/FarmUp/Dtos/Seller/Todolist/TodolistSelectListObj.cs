namespace FarmUp.Dtos.Seller.Todolist
{
    public class ActivitylistSelectObj
    {
        public string ActId { get; set; } = string.Empty;
        public string ActDesc { get; set; } = string.Empty;
    }

    public class ActivitylistSelectObjList
    {
        public ResponseMSG ResponseMSG { get; set; } = new ResponseMSG();
        public List<ActivitylistSelectObj> activitylistSelectObjs { get; set; } = new List<ActivitylistSelectObj>();
    }
}
