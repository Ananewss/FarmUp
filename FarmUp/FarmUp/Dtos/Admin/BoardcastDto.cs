namespace FarmUp.Dtos.Admin
{
    public class LineDto
    {
        public string usr_line_id { get; set; } = string.Empty;
        public string address { get; set; } = string.Empty;

    }

    public class BoardcastDtoList
    {
        public List<LineDto> boardcastDtoList { get; set; } = new List<LineDto>();
    }
}

