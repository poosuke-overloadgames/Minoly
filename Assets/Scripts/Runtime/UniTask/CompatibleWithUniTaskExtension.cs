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
		public static async UniTask<ObjectGetResult> FetchAsync(
			this MinolyDataStore dataStore,
			string className,
			string objectId,
			IProgress<float> progress = null,
			PlayerLoopTiming timing = PlayerLoopTiming.Update,
			CancellationToken cancellationToken = default
		)
		{
			var getter = dataStore.CreateGetter();
			ObjectGetResult result;
			try
			{
				result = await getter.FetchTask(className, objectId, progress, timing, cancellationToken);
			}
			catch (UnityWebRequestException)
			{
				result = getter.GetResult();
			}
			getter.Dispose();
			return result;
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
		public static async UniTask<ObjectPostResult> PostAsync(
			this MinolyDataStore dataStore,
			string className,
			string contentInJson,
			IProgress<float> progress = null,
			PlayerLoopTiming timing = PlayerLoopTiming.Update,
			CancellationToken cancellationToken = default
		)
		{
			var postman = dataStore.CreatePostman();
			ObjectPostResult result;
			try
			{
				result = await postman.PostTask(className, contentInJson, progress, timing, cancellationToken);
			}
			catch (UnityWebRequestException)
			{
				result = postman.GetResult();
			}
			postman.Dispose();
			return result;
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
		public static async UniTask<ObjectUpdateResult> UpdateAsync(
			this MinolyDataStore dataStore,
			string className,
			string objectId,
			string contentInJson,
			IProgress<float> progress = null,
			PlayerLoopTiming timing = PlayerLoopTiming.Update,
			CancellationToken cancellationToken = default
		)
		{
			var updater = dataStore.CreateUpdater();
			ObjectUpdateResult result;
			try
			{
				result = await updater.UpdateTask(className, objectId, contentInJson, progress, timing, cancellationToken);
			}
			catch(UnityWebRequestException)
			{
				result = updater.GetResult();
			}
			updater.Dispose();
			return result;
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
		
		public static async UniTask<ObjectDeleteResult> DeleteAsync(
			this MinolyDataStore dataStore,
			string className,
			string objectId,
			IProgress<float> progress = null,
			PlayerLoopTiming timing = PlayerLoopTiming.Update,
			CancellationToken cancellationToken = default
		)
		{
			var deleter = dataStore.CreateDeleter();
			ObjectDeleteResult result;
			try
			{
				result = await deleter.DeleteTask(className, objectId, progress, timing, cancellationToken);
			}
			catch (UnityWebRequestException)
			{
				result = deleter.GetResult();
			}
			deleter.Dispose();
			return result;
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
		public static async UniTask<ObjectFindResult> FindAsync(
			this MinolyDataStore dataStore,
			string className,
			IEnumerable<IQuery> queries,
			IProgress<float> progress = null,
			PlayerLoopTiming timing = PlayerLoopTiming.Update,
			CancellationToken cancellationToken = default
		)
		{
			var finder = dataStore.CreateFinder();
			
			ObjectFindResult result;
			try
			{
				result = await finder.FindTask(className, queries, progress, timing, cancellationToken);
			}
			catch (UnityWebRequestException)
			{
				result = finder.GetResult();
			}
			finder.Dispose();
			return result;
		}
	}
}

#endif