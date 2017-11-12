    using UnityEngine;

/// <summary>
/// Gizmo を使って法線を可視化します。
/// </summary>
public class DrawNormalWithGizmo : MonoBehaviour
{
    #region Enum

    /// <summary>
    /// 描画の種類。
    /// </summary>
    public enum DrawType
    {
        /// <summary>
        /// 法線を描画しません。
        /// </summary>
        None,

        /// <summary>
        /// 頂点法線を描画します。
        /// </summary>
        Vertex,

        /// <summary>
        /// 面法線を描画します。
        /// </summary>
        Surface
    }

    #endregion Enum

    #region Field

    /// <summary>
    /// 法線描画の種類。
    /// </summary>
    public DrawType drawType = DrawNormalWithGizmo.DrawType.Surface;

    /// <summary>
    /// 法線の長さ。
    /// </summary>
    [Range(0f, 1)]
    public float normalLength = 0.1f;

    /// <summary>
    /// 法線の色。
    /// </summary>
    public Color normalColor = Color.white;

    /// <summary>
    /// 法線の色を方向によって制御するかどうか。
    /// </summary>
    public bool  colorFromDirection = true;

    /// <summary>
    /// メッシュ。
    /// </summary>
    protected Mesh mesh;

    /// <summary>
    /// Transform のキャッシュ。
    /// </summary>
    protected new Transform transform;

    #endregion Field

    #region Method

    /// <summary>
    /// Gizmo の描画時に呼び出されます。
    /// </summary>
    protected virtual void OnDrawGizmos()
    {
        if (this.drawType == DrawType.None)
        {
            return;
        }

        if (this.mesh == null)
        {
            MeshFilter meshFilter = base.gameObject.GetComponent<MeshFilter>();

            if (meshFilter == null)
            {
                this.mesh = base.gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh;
            }
            else
            {
                this.mesh = base.gameObject.GetComponent<MeshFilter>().sharedMesh;
            }
        }

        if (this.transform == null)
        {
            this.transform = base.transform;
        }

        Color previousColor = Gizmos.color;
        Matrix4x4 previousMatrix = Gizmos.matrix;

        Gizmos.matrix = Matrix4x4.TRS(this.transform.position,
                                      this.transform.rotation,
                                      this.transform.localScale);

        switch (this.drawType)
        {
            case DrawType.Surface:
                {
                    DrawSurfaceNormalGizmos();
                    break;
                }
            case DrawType.Vertex:
                {
                    DrawVertexNormalGizmos();
                    break;
                }
        }

        Gizmos.color = previousColor;
        Gizmos.matrix = previousMatrix;
    }

    /// <summary>
    /// 頂点法線の Gizmo を描画します。
    /// </summary>
    protected virtual void DrawVertexNormalGizmos()
    {
        Vector3 [] vertices = this.mesh.vertices;
        Vector3 [] normals  = this.mesh.normals;
        Vector3 normal;

        Gizmos.color = this.normalColor;

        for (int i = 0; i< this.mesh.vertexCount; i++)
        {
            normal = Vector3.Normalize(normals[i]);

            if (this.colorFromDirection)
            {
                Gizmos.color = new Color(normal.x, normal.y, normal.z);
            }

            Gizmos.DrawRay(vertices[i], normal * this.normalLength);
        }
    }

    /// <summary>
    /// 面法線の Gizmo を描画します。
    /// </summary>
    protected virtual void DrawSurfaceNormalGizmos()
    {
        Vector3[] vertices = this.mesh.vertices;
        Vector3[] normals = this.mesh.normals;
        int[] triangles = this.mesh.triangles;

        Vector3 normal;
        Vector3 position;

        int triangleIndex0;
        int triangleIndex1;
        int triangleIndex2;

        Gizmos.color = this.normalColor;

        for (int i = 0; i <= triangles.Length - 3; i += 3)
        {
            triangleIndex0 = triangles[i];
            triangleIndex1 = triangles[i + 1];
            triangleIndex2 = triangles[i + 2];

            position = (vertices[triangleIndex0]
                      + vertices[triangleIndex1]
                      + vertices[triangleIndex2]) / 3;

            normal = (normals[triangleIndex0]
                    + normals[triangleIndex1]
                    + normals[triangleIndex2]) / 3;

            normal = Vector3.Normalize(normal);

            if (this.colorFromDirection)
            {
                Gizmos.color = new Color(normal.x, normal.y, normal.z);
            }

            Gizmos.DrawRay(position, normal * this.normalLength);
        }
    }

    #endregion Method
}