using System.Collections;
using UnityEditor;
using UnityEngine;

public class ImportProcess : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        TextureImporter importer = assetImporter as TextureImporter;
        importer.textureCompression = TextureImporterCompression.Compressed;
        importer.filterMode = FilterMode.Point;
        importer.spritePixelsPerUnit = 1;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
    }
}
