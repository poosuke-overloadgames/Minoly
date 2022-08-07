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
		public static async UniTask<ObjectPostResult> PostTask(
			this ObjectPostman objectPostman,
			string className,
			string contentInJson,
			IProgress<float> progress = null,
			PlayerLoopTiming timing = PlayerLoopTiming.Update,
			CancellationToken cancellationToken = default
		)
		{
			await objectPostman.PostAsync(className, contentInJson).ToUniTask(progress, timing, cancellationToken);
			return objectPostman.GetResult();
		}
	}
}

#endif