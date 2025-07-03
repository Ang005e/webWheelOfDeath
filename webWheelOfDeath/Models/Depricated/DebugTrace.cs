namespace webWheelOfDeath.Models.Depricated
{
    public static class DebugTrace
    {
        private static int _depth = 0;
        private static HashSet<string> _callStack = new HashSet<string>();

        public static void Enter(string methodName)
        {
            _depth++;
            var trace = $"{new string(' ', _depth * 2)}{methodName} (depth: {_depth})";

            if (_depth > 50) // Detect deep recursion
            {
                System.Diagnostics.Debug.WriteLine($"RECURSION DETECTED: {trace}");
                System.Diagnostics.Debug.WriteLine($"Call Stack: {string.Join(" -> ", _callStack)}");
                throw new InvalidOperationException($"Recursion detected at depth {_depth} in {methodName}");
            }

            _callStack.Add(methodName);
            System.Diagnostics.Debug.WriteLine($"ENTER: {trace}");
        }

        public static void Exit(string methodName)
        {
            var trace = $"{new string(' ', _depth * 2)}{methodName}";
            System.Diagnostics.Debug.WriteLine($"EXIT: {trace}");
            _callStack.Remove(methodName);
            _depth--;
        }
    }
}
