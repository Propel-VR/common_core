namespace Salvage.ClothingCuller.Editor.Configuration
{
    public enum RaycastVisibilityOptions
    {
        None = 0,
        VertexOccluded = 1,
        VertexNotOccluded = 2,
        VertexPokingThrough = 4,
        VertexNotPokingThrough = 8,
    }
}