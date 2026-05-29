namespace Meat.Application.Shared.Settings
{
    public class IsApiLocal
    {
        private readonly bool isApiLocal;

        public IsApiLocal(bool _isApiLocal)
        {
            isApiLocal = _isApiLocal;
        }

        public bool GetApiLocal()
        {
            return isApiLocal;
        }
    }
}