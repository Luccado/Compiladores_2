public class Teste {
    public static void main(String[] args) {
        somar();
    }
    public static double somar(){
        double cont;
        double a,b,c;
        cont = 10;
        while(cont > 0) {
            a = lerDouble();
            b = lerDouble();
            if (a > b) {
                c = a - b;
            } else {
                c = b - a;
            }
            System.out.println(c);
            cont = cont - 1;
        }
        return c;
    }
}
