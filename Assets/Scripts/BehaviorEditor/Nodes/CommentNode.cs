using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior.GraphEditor
{
    [CreateAssetMenu(menuName = "Editor/Nodes/Comment Node")]
    public class CommentNode :DrawNode
    {
        private string mComment = "Comment";

        public override void DrawWindow(BaseNode pBaseNode)
        {
            mComment = GUILayout.TextArea(mComment, 200);
        }

        public override void DrawCurve(BaseNode pBaseNode)
        {

        }
    }
}
