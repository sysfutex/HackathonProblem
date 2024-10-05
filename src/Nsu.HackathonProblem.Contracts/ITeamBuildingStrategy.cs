namespace Nsu.HackathonProblem.Contracts
{
    public interface ITeamBuildingStrategy
    {
        /// <summary>
        /// Распределяет тимлидов и джунов по командам
        /// </summary>
        /// 
        /// <param name="teamLeads">Тимлиды</param>
        /// <param name="juniors">Джуны</param>
        /// <param name="teamLeadsWishlists">Вишлисты тимлидов</param>
        /// <param name="juniorsWishlists">Вишлисты джунов</param>
        /// 
        /// <returns>Список команд</returns>
        IEnumerable<Team> BuildTeams(
            IEnumerable<Employee> teamLeads, IEnumerable<Employee> juniors,
            IEnumerable<Wishlist> teamLeadsWishlists, IEnumerable<Wishlist> juniorsWishlists
        );
    }
}
