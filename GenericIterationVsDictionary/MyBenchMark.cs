using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace GenericIterationVsDictionary;

[ShortRunJob(RuntimeMoniker.Mono)]
public class MyBenchMark
{
    [Params(1, 2, 3, 4, 5)] public int CompCountPerType { get; set; }

    [Params(1, 2, 5, 10, 20)] public int CompTypes { get; set; }
    

    private ThingWithComps thing = null!;
    
    [GlobalSetup]
    public void Setup()
    {
        thing = new ThingWithComps(CompCountPerType, CompTypes);
    }
    
    [Benchmark(Baseline = true)]
    public List<ConcreteThingComp0> RimWorldCurrent_ThingComp0()
    {
        return thing.GetComps<ConcreteThingComp0>().ToList();
    }

    [Benchmark]
    public List<ConcreteThingComp0> UsingDictionaryForCache_ThingComp0()
    {
        return thing.GetComps_WithDictionary<ConcreteThingComp0>().ToList();
    }
}

public abstract record ThingComp;
public record ConcreteThingComp0 : ThingComp;
public record ConcreteThingComp1 : ThingComp;
public record ConcreteThingComp2 : ThingComp;
public record ConcreteThingComp3 : ThingComp;
public record ConcreteThingComp4 : ThingComp;
public record ConcreteThingComp5 : ThingComp;
public record ConcreteThingComp6 : ThingComp;
public record ConcreteThingComp7 : ThingComp;
public record ConcreteThingComp8 : ThingComp;
public record ConcreteThingComp9 : ThingComp;
public record ConcreteThingComp10 : ThingComp;
public record ConcreteThingComp11 : ThingComp;
public record ConcreteThingComp12 : ThingComp;
public record ConcreteThingComp13 : ThingComp;
public record ConcreteThingComp14 : ThingComp;
public record ConcreteThingComp15 : ThingComp;
public record ConcreteThingComp16 : ThingComp;
public record ConcreteThingComp17 : ThingComp;
public record ConcreteThingComp18 : ThingComp;
public record ConcreteThingComp19 : ThingComp;
public record ConcreteThingComp20 : ThingComp;
public record ConcreteThingComp21 : ThingComp;
public record ConcreteThingComp22: ThingComp;
public record ConcreteThingComp23 : ThingComp;
public record ConcreteThingComp24 : ThingComp;
public record ConcreteThingComp25 : ThingComp;
public record ConcreteThingComp26 : ThingComp;
public record ConcreteThingComp27 : ThingComp;
public record ConcreteThingComp28 : ThingComp;
public record ConcreteThingComp29 : ThingComp;

public class ThingWithComps
{
    public readonly List<ThingComp> comps;
    
    private readonly Dictionary<Type, object> _compsCache = new();

    private static int _nextId = 1;
    
    public readonly int id = _nextId++;

    public ThingWithComps(int compCountPerType, int compTypes)
    {
        comps = new List<ThingComp>(compCountPerType * compTypes);
        for (var compTypeIndex = 0; compTypeIndex < compTypes; compTypeIndex++)
        {

            var compType = Type.GetType(typeof(ConcreteThingComp0).FullName.Replace("ConcreteThingComp0",
                    $"ConcreteThingComp{compTypeIndex}"),
                true)!;
            
            for (var i = 0; i < compCountPerType; i++)
            {
                comps.Add((ThingComp)Activator.CreateInstance(compType));
            }
        }

        var r = new Random();
        for (var i = 0; i < comps.Count; i++)
        {
            var j = r.Next(0, comps.Count);
            (comps[i], comps[j]) = (comps[j], comps[i]);
        }
    }
    
    
    public IEnumerable<T> GetComps<T>() where T : ThingComp
    {
        if (comps != null!)
        {
            for (var i = 0; i < comps.Count; ++i)
            {
                if (comps[i] is T comp)
                    yield return comp;
            }
        }
    }
    
    public IEnumerable<T> GetComps_WithDictionary<T>() where T : ThingComp
    {
        if (comps != null!)
        {
            var type = typeof(T);
            if (!_compsCache.TryGetValue(type, out var cachedComps))
            {
                cachedComps = comps.OfType<T>().ToList();
                _compsCache.Add(type, cachedComps);
            }
        
            return (List<T>)cachedComps;
        }
        
        return Enumerable.Empty<T>();
    }
}