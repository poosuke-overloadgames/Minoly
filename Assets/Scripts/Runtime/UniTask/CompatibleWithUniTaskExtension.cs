#if VALID_UNITASK

using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Minoly.UniTask
{
	public static class CompatibleWithUniTaskExtension
	{
		public static async UniTask<ObjectGetResult> FetchTask(
			this ObjectGetter objectGetter,
			string className,
			string objectId,
			IProgress<float> progress = null,
			PlayerLoopTiming timing = PlayerLoopTiming.Update,
			CancellationToken cancellationToken = default
		)
		{
			await objectGetter.FetchAsync(className, objectId).ToUniTask(progress, timing, cancellationToken);
			return objectGetter.GetResult();
		}
	}
}

#endif