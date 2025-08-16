// Generic Repository
public class Repository<T>
{
    private List<T> items = new();

    public void Add(T item)
    {
        items.Add(item);
    }

    public List<T> GetAll()
    {
        return items.ToList();
    }

    public T? GetById(Func<T, bool> predicate)
    {
        return items.FirstOrDefault(predicate);
    }

    public bool Remove(Func<T, bool> predicate)
    {
        var item = GetById(predicate);
        if (item != null)
        {
            return items.Remove(item);
        }
        return false;
    }
}

// Patient class
public class Patient
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; }

    public Patient(int id, string name, int age, string gender)
    {
        Id = id;
        Name = name;
        Age = age;
        Gender = gender;
    }

    public override string ToString()
    {
        return $"Patient ID: {Id}, Name: {Name}, Age: {Age}, Gender: {Gender}";
    }
}

// Prescription class
public class Prescription
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public string MedicationName { get; set; }
    public DateTime DateIssued { get; set; }

    public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
    {
        Id = id;
        PatientId = patientId;
        MedicationName = medicationName;
        DateIssued = dateIssued;
    }

    public override string ToString()
    {
        return $"Prescription ID: {Id}, Medication: {MedicationName}, Date: {DateIssued:d}";
    }
}

// Health System Application
public class HealthSystemApp
{
    private Repository<Patient> _patientRepo;
    private Repository<Prescription> _prescriptionRepo;
    private Dictionary<int, List<Prescription>> _prescriptionMap;

    public HealthSystemApp()
    {
        _patientRepo = new Repository<Patient>();
        _prescriptionRepo = new Repository<Prescription>();
        _prescriptionMap = new Dictionary<int, List<Prescription>>();
    }

    public void SeedData()
    {
        // Add sample patients
        _patientRepo.Add(new Patient(1, "John Doe", 35, "Male"));
        _patientRepo.Add(new Patient(2, "Jane Smith", 28, "Female"));
        _patientRepo.Add(new Patient(3, "Bob Johnson", 45, "Male"));

        // Add sample prescriptions
        _prescriptionRepo.Add(new Prescription(1, 1, "Amoxicillin", DateTime.Now.AddDays(-5)));
        _prescriptionRepo.Add(new Prescription(2, 1, "Ibuprofen", DateTime.Now.AddDays(-3)));
        _prescriptionRepo.Add(new Prescription(3, 2, "Paracetamol", DateTime.Now.AddDays(-2)));
        _prescriptionRepo.Add(new Prescription(4, 2, "Aspirin", DateTime.Now.AddDays(-1)));
        _prescriptionRepo.Add(new Prescription(5, 3, "Cetirizine", DateTime.Now));
    }

    public void BuildPrescriptionMap()
    {
        var prescriptions = _prescriptionRepo.GetAll();
        _prescriptionMap = prescriptions
            .GroupBy(p => p.PatientId)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    public List<Prescription> GetPrescriptionsByPatientId(int patientId)
    {
        return _prescriptionMap.TryGetValue(patientId, out var prescriptions) 
            ? prescriptions 
            : new List<Prescription>();
    }

    public void PrintAllPatients()
    {
        Console.WriteLine("\nAll Patients:");
        foreach (var patient in _patientRepo.GetAll())
        {
            Console.WriteLine(patient);
        }
    }

    public void PrintPrescriptionsForPatient(int patientId)
    {
        var patient = _patientRepo.GetById(p => p.Id == patientId);
        if (patient == null)
        {
            Console.WriteLine($"\nNo patient found with ID: {patientId}");
            return;
        }

        Console.WriteLine($"\nPrescriptions for {patient.Name}:");
        var prescriptions = GetPrescriptionsByPatientId(patientId);
        if (prescriptions.Count == 0)
        {
            Console.WriteLine("No prescriptions found.");
            return;
        }

        foreach (var prescription in prescriptions)
        {
            Console.WriteLine(prescription);
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Healthcare System\n");

        // Create and initialize the health system
        var healthSystem = new HealthSystemApp();
        
        // Seed initial data
        healthSystem.SeedData();
        
        // Build the prescription map
        healthSystem.BuildPrescriptionMap();
        
        // Print all patients
        healthSystem.PrintAllPatients();
        
        // Print prescriptions for a specific patient (using ID 1 as an example)
        healthSystem.PrintPrescriptionsForPatient(1);
    }
}
