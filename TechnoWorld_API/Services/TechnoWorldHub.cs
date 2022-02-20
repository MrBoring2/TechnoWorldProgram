using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Threading.Tasks;
using TechnoWorld_API.Helpers;

namespace TechnoWorld_API.Services
{
    public class TechnoWorldHub : Hub
    {
        public static ConcurrentDictionary<string, string> ConnectedUsers = new ConcurrentDictionary<string, string>();

        public override Task OnConnectedAsync()
        {
            var context = this.Context.GetHttpContext();
            var userName = context.User.Identity.Name;
            var roleName = context.User.FindFirst(ClaimsIdentity.DefaultRoleClaimType).Value;

            ConnectedUsers.TryAdd(userName, Context.ConnectionId);
            AddGroupsToClient(userName, roleName);
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(System.Exception exception)
        {
            var context = this.Context.GetHttpContext();
            var userName = context.User.Identity.Name;
            string temp;
            ConnectedUsers.TryRemove(userName, out temp);
            return base.OnDisconnectedAsync(exception);
        }

        private void AddGroupsToClient(string userName, string roleName)
        {
            if (roleName == "terminalUser")
            {
                string connectionId;
                ConnectedUsers.TryGetValue(userName, out connectionId);
                Groups.AddToGroupAsync(connectionId, SignalRGroups.terminal_group);
            }
        }
    }
}
