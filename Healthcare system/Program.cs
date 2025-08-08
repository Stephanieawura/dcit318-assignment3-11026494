using System;
using System.Collections.Generic;
using System.Linq; // Needed for FirstOrDefault & GroupBy

// --------------------------
// Generic Repository
// --------------------------
public class Repository<T>
{
    private readonly List<T> items = new();

    // Add an item
    public void Add(T item) => items.Add(item);

    // Return all items
    public List<T> GetAll() => new List<T>(items);

    // Return first match or null
    public T? GetById(Func<T, bool> predicate) => items.FirstOrDefault(predicate);

    // Remove first item matching predicate; return true if removed
    public bool Remove(Func<T, bool> predicate)
    {
        var item = items.FirstOrDefault(predicate);
        if (item != null)
        {
            items.Remove(item);
            return true;
        }
        return false;
    }
}

// --------------------------
// Domain models
// --------------------------
public class Patient
{
    public int Id { get; }
    public string Name { get; }
    public int Age { get; }
    public string Gender { get; }

    public Patient(int id, string name, int age, string gender)
    {
        Id = id;
        Name = name;
        Age = age;
        Gender = gender;
    }

    public override string ToString() => $"[{Id}] {Name}, Age: {Age}, Gender: {Gender}";
}

public class Prescription
{
    public int Id { get; }
    public int PatientId { get; }
    public string MedicationName { get; }
    public DateTime DateIssued { get; }

    public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
    {
        Id = id;
        PatientId = patientId;
        MedicationName = medicationName;
        DateIssued = dateIssued;
    }

    public override string ToString() => $"Presc[{Id}] {MedicationName} (Issued: {DateIssued:d})";
}

// --------------------------
// HealthSystemApp
// --------------------------
public class HealthSystemApp
{
    private readonly Repository<Patient> _patientRepo = new();
    private readonly Repository<Prescription> _prescriptionRepo = new();
    private Dictionary<int, List<Prescription>> _prescriptionMap = new();

    // Seed sample patients and prescriptions
    public void SeedData()
    {
        // Patients
        _patientRepo.Add(new Patient(1, "Alice Smith", 30, "Female"));
        _patientRepo.Add(new Patient(2, "Kwame Mensah", 45, "Male"));
        _patientRepo.Add(new Patient(3, "Esi Adu", 22, "Female"));

        // Prescriptions
        _prescriptionRepo.Add(new Prescription(1, 1, "Amoxicillin 500mg", DateTime.Now.AddDays(-10)));
        _prescriptionRepo.Add(new Prescription(2, 1, "Paracetamol 500mg", DateTime.Now.AddDays(-5)));
        _prescriptionRepo.Add(new Prescription(3, 2, "Lisinopril 10mg", DateTime.Now.AddDays(-20)));
        _prescriptionRepo.Add(new Prescription(4, 3, "Cetirizine 10mg", DateTime.Now.AddDays(-2)));
        _prescriptionRepo.Add(new Prescription(5, 2, "Simvastatin 20mg", DateTime.Now.AddDays(-1)));
    }

    // Build the dictionary map: patientId -> list of prescriptions
    public void BuildPrescriptionMap()
    {
        var allPrescriptions = _prescriptionRepo.GetAll();
        _prescriptionMap = allPrescriptions
            .GroupBy(p => p.PatientId)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    // Get prescriptions by patient id (returns empty list if none)
    public List<Prescription> GetPrescriptionsByPatientId(int patientId)
    {
        return _prescriptionMap.TryGetValue(patientId, out var list) ? new List<Prescription>(list) : new List<Prescription>();
    }

    // Print all patients
    public void PrintAllPatients()
    {
        var patients = _patientRepo.GetAll();
        Console.WriteLine("All patients:");
        foreach (var p in patients)
        {
            Console.WriteLine($"  {p}");
        }
    }

    // Print prescriptions for a specific patient
    public void PrintPrescriptionsForPatient(int patientId)
    {
        var patient = _patientRepo.GetById(p => ((Patient)(object)p).Id == patientId);
        if (patient == null)
        {
            Console.WriteLine($"No patient found with ID {patientId}");
            return;
        }

        Console.WriteLine($"\nPrescriptions for {((Patient)(object)patient).Name} (ID {patientId}):");
        var prescriptions = GetPrescriptionsByPatientId(patientId);
        if (prescriptions.Count == 0)
        {
            Console.WriteLine("  (No prescriptions found)");
            return;
        }

        foreach (var presc in prescriptions)
        {
            Console.WriteLine($"  {presc}");
        }
    }
}

// --------------------------
// Program entry point
// --------------------------
class Program
{
    static void Main()
    {
        var app = new HealthSystemApp();

        // Main flow
        app.SeedData();
        app.BuildPrescriptionMap();

        app.PrintAllPatients();

        Console.WriteLine("\nEnter a patient ID to view prescriptions (e.g., 1): ");
        if (int.TryParse(Console.ReadLine(), out int selectedId))
        {
            app.PrintPrescriptionsForPatient(selectedId);
        }
        else
        {
            Console.WriteLine("Invalid input. Showing prescriptions for Patient ID 1 by default.\n");
            app.PrintPrescriptionsForPatient(1);
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
