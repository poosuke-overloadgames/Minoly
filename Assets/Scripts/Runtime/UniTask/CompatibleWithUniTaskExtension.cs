#if VALID_UNITASK

using System;
using System.Collections.Generic;
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
		public static async UniTask<ObjectUpdateResult> UpdateTask(
			this ObjectUpdater objectUpdater,
			string className,
			string objectId,
			string contentInJson,
			IProgress<float> progress = null,
			PlayerLoopTiming timing = PlayerLoopTiming.Update,
			CancellationToken cancellationToken = default
		)
		{
			await objectUpdater.UpdateAsync(className, objectId, contentInJson).ToUniTask(progress, timing, cancellationToken);
			return objectUpdater.GetResult();
		}
		public static async UniTask<ObjectDeleteResult> DeleteTask(
			this ObjectDeleter objectDeleter,
			string className,
			string objectId,
			IProgress<float> progress = null,
			PlayerLoopTiming timing = PlayerLoopTiming.Update,
			CancellationToken cancellationToken = default
		)
		{
			await objectDeleter.DeleteAsync(className, objectId).ToUniTask(progress, timing, cancellationToken);
			return objectDeleter.GetResult();
		}
		
		public static async UniTask<ObjectFindResult> FindTask(
			this ObjectFinder objectFinder,
			string className,
			IEnumerable<IQuery> queries,
			IProgress<float> progress = null,
			PlayerLoopTiming timing = PlayerLoopTiming.Update,
			CancellationToken cancellationToken = default
		)
		{
			await objectFinder.FindAsync(className, queries).ToUniTask(progress, timing, cancellationToken);
			return objectFinder.GetResult();
		}
	}
}

#endif