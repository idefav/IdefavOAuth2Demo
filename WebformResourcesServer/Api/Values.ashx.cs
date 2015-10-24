using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using WebformResourcesServer.Code;

namespace WebformResourcesServer.Api
{
    /// <summary>
    /// Values 的摘要说明
    /// </summary>
    public class Values : IHttpAsyncHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            return new AsyncResult(cb, extraData, new AshxHandler(context).Proc(c =>
            {
                c.Response.Write("The Data you get!");
            }));


        }

        public void EndProcessRequest(IAsyncResult result)
        {
            var r = (AsyncResult)result;
            r.Task.Wait();

        }
    }

    internal class AsyncResult : IAsyncResult
    {
        private object _state;
        private Task _task;
        private bool _completedSynchronously;

        public AsyncResult(AsyncCallback callback, object state, Task task)
        {
            _state = state;
            _task = task;
            _completedSynchronously = _task.IsCompleted;
            _task.ContinueWith(t => callback(this), TaskContinuationOptions.ExecuteSynchronously);
        }

        public Task Task
        {
            get { return _task; }
        }


        public object AsyncState
        {
            get { return _state; }
        }

        public WaitHandle AsyncWaitHandle
        {
            get { return ((IAsyncResult)_task).AsyncWaitHandle; }
        }

        public bool CompletedSynchronously
        {
            get { return _completedSynchronously; }
        }

        public bool IsCompleted
        {
            get { return _task.IsCompleted; }
        }
    }
}