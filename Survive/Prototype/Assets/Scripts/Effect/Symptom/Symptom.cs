namespace Effect.Symptoms
{
    
    public abstract class Symptom
    {
        protected PlayerBody playerBody;

        public Symptom(PlayerBody player)
        {
            playerBody = player;
        }

        public abstract void Apply();
        public abstract void StartSymptom();
        public abstract void UpdateSymptom();
        public abstract void StopSymptom();
    }
}
public enum BaseSymptomType
{
    None,
    Dizziness,
    Vomit,
    Hallucination,
    Unconscious
}