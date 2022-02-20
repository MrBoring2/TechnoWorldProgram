using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace TechnoWorld_API.Services
{
    public class TechnoWorldHub : Hub
    {
        public static ConcurrentDictionary<string, string> ConnectedUsers = new ConcurrentDictionary<string, string>();
       
        public override Task OnConnectedAsync()
        {
            var context = this.Context.GetHttpContext();
            var userName = context.User.Identity.Name;
            //ConnectedUsers.TryAdd(userName, Context.ConnectionId);
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(System.Exception exception)
        {
            var context = this.Context.GetHttpContext();
            var userName = context.User.Identity.Name;
            string temp;
            //ConnectedUsers.TryRemove(userName, out temp);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
