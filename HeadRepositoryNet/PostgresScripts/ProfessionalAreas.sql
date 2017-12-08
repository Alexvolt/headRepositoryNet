-- Sequence: public.users_id_seq

-- DROP SEQUENCE public.users_id_seq;

CREATE SEQUENCE public.ProfessionalAreas_id_seq
  INCREMENT 1
  MINVALUE 1
  MAXVALUE 9223372036854775807
  START 1
  CACHE 1;
ALTER SEQUENCE public.ProfessionalAreas_id_seq
  OWNER TO postgres;

-- Table: public."ProfessionalAreas"

-- DROP TABLE public."ProfessionalAreas";

CREATE TABLE public."ProfessionalAreas"
(
  "Id" integer NOT NULL DEFAULT nextval('ProfessionalAreas_id_seq'::regclass),
  "ParentId" integer NULL,
  "Name" character varying(255) NOT NULL,
  "Created_at" timestamp with time zone NOT NULL DEFAULT now(),
  CONSTRAINT ProfessionalAreas_pkey PRIMARY KEY ("Id"),
  CONSTRAINT ProfessionalAreas_Name_unique UNIQUE ("Name")
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public."ProfessionalAreas"
  OWNER TO postgres;
