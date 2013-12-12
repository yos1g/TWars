#pragma strict
function Update () {

}

@ExecuteInEditMode
function OnDrawGizmos() {
	Gizmos.DrawLine(this.transform.position, this.transform.forward);
}