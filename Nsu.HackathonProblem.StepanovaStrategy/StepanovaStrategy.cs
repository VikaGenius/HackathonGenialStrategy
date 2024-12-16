using Nsu.HackathonProblem.Contracts;

namespace Nsu.HackathonProblem.StepanovaStrategy;

public class StepanovaStrategy : ITeamBuildingStrategy
{
	public IEnumerable<Team> BuildTeams(
		IEnumerable<Employee> teamLeads,
		IEnumerable<Employee> juniors,
		IEnumerable<Wishlist> teamLeadsWishlists,
		IEnumerable<Wishlist> juniorsWishlists) 
	{
		var teamLeadsList = teamLeads.ToList();
		var juniorsList = juniors.ToList();

		int teamLeadsCount = teamLeadsList.Count;
		int juniorsCount = juniorsList.Count;

		var teamLeadIdToIndex = teamLeadsList.Select((e, idx) => new { e.Id, idx })
											.ToDictionary(x => x.Id, x => x.idx);
		var juniorIdToIndex = juniorsList.Select((e, idx) => new { e.Id, idx })
											.ToDictionary(x => x.Id, x => x.idx);

		var teamLeadWishlists = teamLeadsWishlists.ToDictionary(w => w.EmployeeId, w => w.DesiredEmployees);
		var juniorWishlists = juniorsWishlists.ToDictionary(w => w.EmployeeId, w => w.DesiredEmployees);

		var costMatrix = new int[teamLeadsCount, juniorsCount];

		for (int i = 0; i < teamLeadsCount; i++)
		{
			for (int j = 0; j < juniorsCount; j++)
			{
				var teamLead = teamLeadsList[i];
				var junior = juniorsList[j];

				var teamLeadWishlist = teamLeadWishlists.ContainsKey(teamLead.Id) ? teamLeadWishlists[teamLead.Id] : [];
				var juniorWishlist = juniorWishlists.ContainsKey(junior.Id) ? juniorWishlists[junior.Id] : [];
				int total = GetSatisfactionIndex(teamLeadWishlist, junior.Id) + GetSatisfactionIndex(juniorWishlist, teamLead.Id);

				costMatrix[i, j] = -total;
			}
		}

		return TeamBuilding(teamLeadsList, juniorsList, HungarianAlgorithm.HungarianAlgorithm.FindAssignments(costMatrix));
	}

	private static IEnumerable<Team> TeamBuilding(List<Employee> teamLeads, List<Employee> juniors, int[] assignments)
    {
        var teams = new List<Team>();
        for (var teamLeadIndex = 0; teamLeadIndex < assignments.Length; ++teamLeadIndex)
        {
            var juniorIndex = assignments[teamLeadIndex];
            if (juniorIndex >= 0 && juniorIndex < juniors.Count)
            {
                teams.Add(new Team(teamLeads[teamLeadIndex], juniors[juniorIndex]));
            }
        }

        return teams;
    }

	private int GetSatisfactionIndex(int[] wishlist, int employeeId)
	{
		int index = Array.IndexOf(wishlist, employeeId);
		return index >= 0 ? wishlist.Length - index : 0;
	}

}