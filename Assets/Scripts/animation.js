

function OnTriggerEnter (other : Collider) {
    GetComponent.<Animation>().Play("open");
    yield WaitForSeconds (1.75);
    GetComponent.<Animation>().Play("close");
}
