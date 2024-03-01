namespace Tapxyc.UI
{
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.UI;

    //обратная маска
    //
    //Сделать объект на канвасе, пометить на него Image и Mask, у маски убрать отрисовку, у Image поставить что-нибудь в спрайт
    //Создать дочерний объект, поместить на него этот скрипт и растянуть за границы родительского объекта
    //ОБЪЕКТ С MASK ДОЛЖЕН БЫТЬ МЕНЬШЕ ДОЧЕРНЕГО! ИМЕННО ОН И ВИДЕН!

    public class CutOutMask : Image
    {

        public override Material materialForRendering
        {
            get
            {
                Material material = new Material(base.materialForRendering);
                material.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
                return material;
            }
        }
    }
}
