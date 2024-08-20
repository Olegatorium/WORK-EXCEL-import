My Task:

1. **Import multilines**: This means that the entire WORK is described in multiple lines – all lines share the same Sender Work Code in column A.
2. **Group all lines** that have the same Sender Work Code in column A.
   2.1. Check if there is a WORK in our database with the following record: Type - S, Name - ZAIKS, ID - 97, and the given Sender Work Code. 
   - If it exists, output "WORK already in database, omitted" in the report, skip the import, and proceed to the next one.
   - If not, continue the import process.
   2.2. Check if there is a WORK in our database with the same ISWC (value in column J).
   - If it exists, output "WORK already in database, omitted" in the report, skip the import, and proceed to the next one.
   - If not, continue the import process.
3. If the value in column B (Record Code) is **T**:
   3.a. Enter the value from column C (Title) into the **TITLE** field (Mandatory).
   3.b. If the value in column E (Shareholder) is **"P"**, enter "PL" into the **LANGUAGE** field; if the value is different, leave the field empty (Optional).
   3.c. Enter the value from column J (ISWC) into the **ISWC** field (Optional).
   - The lines are arranged by Record Code – first always **T**, then optionally **D**, followed by **U**. T is Mandatory, U is Mandatory, D is Optional.
4. If the value in column B (Record Code) is **D**:
   - Enter the value from column C (Title) into the **AKA TITLE** field.
5. Check if, within a single transaction and under the same Sender Work Code, there is more than one line **U** with the same value in column F (IPI Name Number). If so:
   5.a. If the value in column D (Role) is **C** and **A**, change it to **CA**; if it is **C** and **AD**, change it to **CA**; if it is **A** and **AR**, change it to **CA**; if it is **A** and **AD**, change it to **A**; if it is **C** and **AR**, change it to **C**.
   5.b. Sum the values in column G (InWork PR).
   5.c. If column I (Controlled) is empty for all lines with the same value in column F (IPI Name Number), proceed; if not, output the error "No uncontrolled for all lines with the same IPI Name Number".
   5.d. If column I (Controlled) has the value **Y** for all lines with the same value in column F (IPI Name Number), proceed; if not, output the error "No controlled for all lines with the same IPI Name Number".
   5.e. If column K (AGREEMENT NO) has the same value for all lines with the same value in column F (IPI Name Number), proceed; if not, output the error "No Agreement for all lines with the same IPI Name Number".
6. If the value in column B (Record Code) is **U**:
   - Check if there is a rightsholder in our database with the IPI number listed in column F. If yes, use that; if not, create a new RIGHTSHOLDER record using the value in column E (first part as SECOND NAME, second and third parts as NAME) and the value in column F (IPI NAME NUMBER).
   6.a. If column I (Controlled) is empty, it means the value is **OWR** – an uncontrolled author. In this case, use the value in column G (InWork PR) as InWork and fill in the SHARES accordingly.
   - If the value in column B (Record Code) is **U** and there is no line where column I (Controlled) has the value **Y**, output the error "No SWR in transaction", skip this WORK, and proceed to the next one.
   6.b. If column I (Controlled) has the value **Y**, it means the value is **SWR** – a controlled author. In this case, use the value in column G (InWork PR) as InWork and the value in column K (AGREEMENT NO) to fill in the SHARES accordingly based on the agreement in the database and the calculation.
   6.c. If the value in column I is "Y", the value in column K (AGREEMENT NO) is mandatory. If column K is missing, output the error "No agreement with FinalSE".

**Note**: In all error reports, use the Sender Work Code as the transaction identifier where the error occurred.  
**IMPORTANT**: When importing, convert characters to ASCII.
