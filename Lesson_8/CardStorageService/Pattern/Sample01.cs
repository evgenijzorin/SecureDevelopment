namespace Pattern;
class Programm
{
    static void Main1(string[] args)
    {
        #region Adapter
        // В примере для того чтобы использовать метод Request класса MyClient принимающего объект Target, перадав ему объект SpecificTarget,
        // Мы создаем объект обертку Adapter унаследованный от Target и переопределяем в нем метод Request вызывая в нем SpecificRequest у объекта
        // specificRequest создав его дополнительным полем.
        var myClient = new MyClient();
        myClient.Request(new Target());
        // используем класс обертку
        myClient.Request(new Adapter());
        #endregion

        #region Facade
        // Паттерн Fasade - структурный шаблон проектирования, позволяющий скрыть сложность системы
        // путём сведения всех возможных внешних вызовов к одному объекту,
        // делегирующему их соответствующим объектам системы.
        Facade fasade = new Facade(new SystemA(), new SystemB(), new SystemC());
        #endregion

        #region Decorator
        // Декоратор (англ. Decorator) — структурный шаблон проектирования, предназначенный для динамического подключения дополнительного
        // поведения к объекту. Шаблон Декоратор предоставляет гибкую альтернативу практике создания подклассов с целью расширения
        // функциональности.
        FinalWorkComponent1 finalWorkComponent1 = new FinalWorkComponent1();
        finalWorkComponent1.DoWork();

        finalWorkComponent1.SetComponent(new WorkComponent1());
        finalWorkComponent1.DoWork();
        finalWorkComponent1.SetComponent(new WorkComponent2());
        finalWorkComponent1.DoWork();

        #endregion

        Console.ReadKey();

    }
}

#region Adapter
class MyClient
{
    public void Request(Target target)
    {
        target.Request();
    }
}

class Target
{
    public virtual void Request()
    {
        Console.WriteLine("Do some work (1)  ....");
    }
}


// Класс адптер - обертка таржета  в SpecificTarget
class Adapter : Target
{
    private SpecificTarget specificTarget = new SpecificTarget();
    public override void Request()
    {
        specificTarget.SpecificRequest();
    }
}


class SpecificTarget
{
    public void SpecificRequest()
    {
        Console.WriteLine("Do some work (2)  ....");
    }
}
#endregion

#region Facade
public class SystemA
{
    public void ProcessA ()
    {
    }
    public void ProcessA1()
    {
    }
    public void ProcessA2()
    {
    }
    public void ProcessA3()
    {
    }
}

public class SystemB
{
    public void ProcessB()
    {
    }
    public void ProcessB1()
    {
    }
    public void ProcessB2()
    {
    }
}

public class SystemC
{
    public void ProcessC()
    {
    }
    public void ProcessC1()
    {
    }

}

public class Facade
{
    private SystemA _systemA;
    private SystemB _systemB;
    private SystemC _systemC;
    public Facade(SystemA systemA, SystemB systemB, SystemC systemC)
    {
        _systemA=systemA;
        _systemB=systemB;
        _systemC=systemC;
    }
    public void DoWork1()
    {
        _systemA.ProcessA1(); 
    }
    public void DoWork2()
    {
        _systemA.ProcessA2();
    }
    public void DoWork3()
    {
        _systemA.ProcessA3();
    }
}




#endregion

#region Decorator

abstract class BaseComponent
{
    public abstract void DoWork();
}
class WorkComponent1 : BaseComponent
{
    public override void DoWork()
    {
        Console.WriteLine("Do work 1 .... ");
    }
}
class WorkComponent2 : BaseComponent
{
    public override void DoWork()
    {
        Console.WriteLine("Do work 2 .... ");
    }
}
abstract class WorkDecoratorComponent : BaseComponent
{
    protected BaseComponent baseComponent;
    public void SetComponent(BaseComponent baseComponent)
    {
        this.baseComponent = baseComponent;
    }
    public override void DoWork()
    {
        if (baseComponent != null)            
            baseComponent.DoWork();
    }
}
class FinalWorkComponent1 : WorkDecoratorComponent
{    public override void DoWork()
    {
        if (baseComponent != null)
            baseComponent.DoWork();
        else
            Console.WriteLine("Final (default) doWork (1) ...");
    }
}
#endregion

