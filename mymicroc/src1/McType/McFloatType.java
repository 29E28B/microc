package McType;

public class McFloatType extends McBaseType {
    private float value;


    public McFloatType(){
        this.value = 0;
    }

    public McFloatType(float value){
        this.value = value;
    }

    public float getValue() {
        return value;
    }

    public void setValue(float value) {
        this.value = value;
    }
}
