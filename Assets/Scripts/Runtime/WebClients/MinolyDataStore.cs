namespace Minoly
{
	public class MinolyDataStore
	{
		private readonly string _applicationKey;
		private readonly string _clientKey;
		private readonly ICurrentDateTime _currentDateTime;
		public MinolyDataStore(string applicationKey, string clientKey, ICurrentDateTime currentDateTime = null)
		{
			_applicationKey = applicationKey;
			_clientKey = clientKey;
			_currentDateTime = currentDateTime;
		}

		public ObjectGetter CreateGetter() => new ObjectGetter(_applicationKey, _clientKey, _currentDateTime);
		public ObjectPostman CreatePostman() => new ObjectPostman(_applicationKey, _clientKey, _currentDateTime);
		public ObjectUpdater CreateUpdater() => new ObjectUpdater(_applicationKey, _clientKey, _currentDateTime);
		public ObjectFinder CreateFinder() => new ObjectFinder(_applicationKey, _clientKey, _currentDateTime);
		public ObjectDeleter CreateDeleter() => new ObjectDeleter(_applicationKey, _clientKey, _currentDateTime);
	}
}