public class ActorViewModel
{
    public int PersonId { get; set; }
    public string PersonName { get; set; }
    public int MovieCount { get; set; }
    public List<MovieRoleViewModel> Movies { get; set; }
}