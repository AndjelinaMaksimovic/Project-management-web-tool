namespace Codedberries.Models.DTOs
{
    public class StatusOrderChangeDTO
    {
        public int ProjectId { get; set; }  // id of project that contains statuses where order needs to be changed
        public List<int> NewOrder { get; set; }
        /*
         * list of statuses ids for new order, for example:
         *  - we currently have status order:  [1, 2, 3, 4]
         *  - we want to change it to:         [3, 2, 1, 4]
         *  therefore NewOrder should contain: [3, 2, 1, 4]
         */
    }
}
