-- Sequence: public.users_id_seq

-- DROP SEQUENCE public.users_id_seq;

CREATE SEQUENCE public."Users_id_seq"
    INCREMENT 1
    START 1
    MINVALUE 1
    MAXVALUE 9223372036854775807
    CACHE 1;
ALTER SEQUENCE public."Users_id_seq"
    OWNER TO postgres;

-- Table: public."ProfessionalAreas"

-- DROP TABLE public."ProfessionalAreas";

CREATE TABLE public."Users"
(
  Id integer NOT NULL DEFAULT nextval('"Users_id_seq"'::regclass),
  "Username" character varying(255) NOT NULL,
  "FirstName" character varying(255) NOT NULL,
  "LastName" character varying(255) NOT NULL,
  "Password" character varying(255) NOT NULL,
  "Email" character varying(255) NOT NULL,
  "Admin" boolean NOT NULL DEFAULT false,
  "HaveAccess" boolean NOT NULL DEFAULT false,
  "Created_at" timestamp with time zone NOT NULL DEFAULT now(),
  CONSTRAINT "Users_pkey" PRIMARY KEY (id),
  CONSTRAINT users_username_unique UNIQUE ("Username")
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public."Users"
  OWNER TO postgres;

