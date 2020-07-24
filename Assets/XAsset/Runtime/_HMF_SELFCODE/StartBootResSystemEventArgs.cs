

namespace Hegametech.Framework
{
	/// <summary>
	/// 开始启动资源系统（资源更新完毕或无需更新）事件。
	/// </summary>
	public sealed class StartBootResSystemEventArgs
	{
		/// <summary>
		/// 开始启动资源系统（资源更新完毕或无需更新）事件编号。
		/// </summary>
		public static readonly int EventId = typeof(StartBootResSystemEventArgs).GetHashCode();

		/// <summary>
		/// 初始化开始启动资源系统（资源更新完毕或无需更新）事件的新实例。
		/// </summary>
		public StartBootResSystemEventArgs()
		{
			ResState = 0;
		}

		/// <summary>
		/// 获取开始启动资源系统（资源更新完毕或无需更新）事件编号。
		/// </summary>
		//public override int Id { get { return EventId; } }

		/// <summary>
		/// 资源状态(0-无需更新 1-更新完成)
		/// </summary>
		public int ResState { get; private set; }

		/// <summary>
		/// 创建开始启动资源系统（资源更新完毕或无需更新）事件。
		/// </summary>
		/// <param name="resState">(0-无需更新 1-更新完成)</param>
		/// <returns>创建的开始启动资源系统（资源更新完毕或无需更新）事件。</returns>ram>
		public static StartBootResSystemEventArgs Create(int resState)
		{
			StartBootResSystemEventArgs startBootResSystemEventArgs = null;
			return startBootResSystemEventArgs;
		}

		/// <summary>
		/// 清理开始启动资源系统（资源更新完毕或无需更新）事件。
		/// </summary>
		public void Clear()
		{
			ResState = 0;
		}
	}

	public class Log
    {
		public static void Error(string s)
        {

        }
    }

    public class FrameworkBoot 
	{
		public static EventFire Event;
	}

	public class EventFire
    {
		public void Fire(int id, StartBootResSystemEventArgs a)
        {

        }
    }

}
