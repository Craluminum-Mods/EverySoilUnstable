using Newtonsoft.Json.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

[assembly: ModInfo("Every Soil Unstable")]

namespace EverySoilUnstable;

public class Core : ModSystem
{
    public override void AssetsFinalize(ICoreAPI api)
    {
        foreach (var block in api.World.Blocks)
        {
            if (block.BlockMaterial is not (EnumBlockMaterial.Soil or EnumBlockMaterial.Gravel or EnumBlockMaterial.Sand))
            {
                continue;
            }

            if (block.HasBehavior<BlockBehaviorUnstableFalling>())
            {
                continue;
            }

            var properties = new { fallSound = "effect/rockslide", fallSideways = true, dustIntensity = 0.25 };
            AppendBlockBehavior<BlockBehaviorUnstableFalling>(new(block), properties);

            block.Attributes ??= new JsonObject(new JObject());
            block.Attributes.Token["allowUnstablePlacement"] = JToken.FromObject(true);
        }

        api.World.Logger.Event("started 'Every Soil Unstable' mod");
    }

    private void AppendBlockBehavior<T>(T instance, object propertiesFromJson) where T : BlockBehavior
    {
        instance.Initialize(new JsonObject(JToken.FromObject(propertiesFromJson)));
        instance.block.CollectibleBehaviors = instance.block.CollectibleBehaviors.Append(instance);
        instance.block.BlockBehaviors = instance.block.BlockBehaviors.Append(instance);
    }
}