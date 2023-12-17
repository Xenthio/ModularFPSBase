using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPSKit;

public class InventoryComponent : Component
{
	public GameObject ActiveItem;
	public List<GameObject> Items = new List<GameObject>();
}
