using _Scripts.Utilities;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/ValueStore/ULong Value")]
public class ULongValue : ValueStore<ulong> { }
[CreateAssetMenu(menuName = "Scriptable Objects/ValueStore/UInt Value")]
public class UIntValue : ValueStore<uint> { }
[CreateAssetMenu(menuName = "Scriptable Objects/ValueStore/Int Value")]
public class IntValue : ValueStore<int> { }
[CreateAssetMenu(menuName = "Scriptable Objects/ValueStore/Long Value")]
public class LongValue : ValueStore<long> { }
[CreateAssetMenu(menuName = "Scriptable Objects/ValueStore/String Value")]
public class StringValue : ValueStore<string> { }
[CreateAssetMenu(menuName = "Scriptable Objects/ValueStore/UpgradeItems Value")]
public class UpgradeItemsValue : ValueStore<string> { }